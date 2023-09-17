# IAE.TradingDesk.Persistence

## In order to update the dictionaries (seed) data in database, you must perform the following steps:

### I. Make all the necessary changes to the files located in the SeedData folder, taking into account the following conditions:

#### 1. There should be no duplication of ID fields.

#### 2. There should be no change to already installed ID values.

#### 3. When adding new records, you must manually increment ID values.

### II. Create migration:

#### 1. Migration can be empty if there were no other entities changes.

#### 2. Add the following line of code to the "Up" method: "migrationBuilder.InsertOrUpdateAllDictionaries();".

#### 3. If dictionaries data are initialized for the first time, then you can (optionally) add the following line of code to the "Down" method: "migrationBuilder.DropAllDictionaries();".

### III. Apply migration or run IAE.TradingDesk.Api (migration will be applied automatically):

#### 1. Applying migration to insert or update dictionaries data can take approximately a few of minutes.

#### 2. You can use the console to track the process of applying the migration.

#### 3. Until the process of applying the migration ends, the API will not start.

#### 4. To apply the migration, a transaction is used, so if any errors occur during the application, then no changes will be made to the database.

#### 5. If there are any errors when applying the migration, then you need to check the changes made to the files located in the SeedData folder again, especially pay attention to ID values.