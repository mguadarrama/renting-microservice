# Spec: vehicle renting microservice

What the service must do, how each rule is enforced and which test checks it. The README explains how to run it
and the design.

## Scope

A renting company manages its fleet through a REST microservice built on the provided hexagonal architecture template.

In scope:

1. Add new vehicles to the fleet.
2. List the vehicles available for rent.
3. Rent a vehicle.
4. Return a vehicle.

Out of scope: customer management (a customer is identified only by an id), pricing, reservations for future dates,
rental history queries, and authentication (the template's setup is kept, endpoints are not protected).

## Business rules

- **R1. One rental per customer.** A customer cannot have more than one rented vehicle at the same time.
- **R2. Fleet age limit.** The fleet cannot contain vehicles manufactured more than 5 years ago.
  - A vehicle exactly 5 years old is still accepted.
  - A vehicle that ages out after being added is no longer listed as available and cannot be rented.
- **R3. Unique license plate.** Two vehicles in the fleet cannot share a license plate.
- **R4. Rental lifecycle.** A rented vehicle cannot be rented again until it is returned; a vehicle that is not rented
  cannot be returned.

## Acceptance criteria

| # | Given | When | Then |
|---|---|---|---|
| A1 | Valid data, manufactured within 5 years | `POST /api/vehicles` | 201, vehicle stored |
| A2 | Manufactured more than 5 years ago (R2) | `POST /api/vehicles` | 400, nothing stored |
| A3 | Manufacture date in the future | `POST /api/vehicles` | 400 |
| A4 | Missing or invalid fields | `POST /api/vehicles` | 400 (model validation at host level) |
| A5 | Plate already in the fleet (R3) | `POST /api/vehicles` | 409 |
| A6 | Some vehicles rented or aged out | `GET /api/vehicles/available` | 200, only vehicles that can be rented now |
| A7 | Available vehicle, customer without a rental | `POST /api/vehicles/{id}/rental` | 200, vehicle rented to the customer |
| A8 | Customer already has a rented vehicle (R1) | `POST /api/vehicles/{id}/rental` | 409, nothing saved |
| A9 | Vehicle already rented (R4) | `POST /api/vehicles/{id}/rental` | 409, nothing saved |
| A10 | Vehicle aged out after being added (R2) | `POST /api/vehicles/{id}/rental` | 400 |
| A11 | Vehicle does not exist | `POST` or `DELETE /api/vehicles/{id}/rental` | 404 |
| A12 | Rented vehicle | `DELETE /api/vehicles/{id}/rental` | 200, vehicle available again |
| A13 | Vehicle not rented (R4) | `DELETE /api/vehicles/{id}/rental` | 409 |
| A14 | Two concurrent requests that would break R1 or R3, or rent the same vehicle twice | Both reach the database | Only one succeeds, the other gets 409 |

## Where each rule is enforced

| Rule | Domain | Use case | Database |
|---|---|---|---|
| R1 | | `RentVehicleUseCase` checks the repository (the rule spans several aggregates) | Partial unique index `ux_rentedBy` on rented vehicles |
| R2 | `Vehicle.Create`, `Vehicle.Rent`, `Vehicle.IsAvailable` | | |
| R3 | | `CreateVehicleUseCase` | Unique index `ux_licensePlate` |
| R4 | `Vehicle.Rent`, `Vehicle.Return` | | Optimistic concurrency version on update |

The database checks back up the domain and use case checks for concurrent requests (A14). "Now" comes from
`TimeProvider`, so rules depending on the date are deterministic in tests.

## Tests required by the brief

| Type | What the brief asks | Example in this repo |
|---|---|---|
| Unit | One method, without its dependencies | `test/unit`: create and rent use cases with mocked ports (A1, A2, A5, A7, A8, A9, A11) |
| Infrastructure | One REST method, host level only: receive the call and validate the model | `test/infrastructure`: `PostVehicleWithMissingFieldsReturnsBadRequest` (A4) |
| Functional | Integration test excluding the host | `test/functional`: `CustomerCannotRentASecondVehicleWhileHavingOne` against MongoDB (A8) |

## Constraints

- Built on the provided template, keeping its folder structure and patterns (use cases, output ports and presenters,
  MediatR, Unit of Work).
- Runs locally with only Docker: `docker compose up --build`.
