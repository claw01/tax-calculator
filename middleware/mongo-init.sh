#!/bin/bash
set -e
mongo <<EOF
conn = new Mongo();

db = conn.getDB("$MONGO_DB_NAME");

db.createUser({
  user: "$MONGO_INITDB_ROOT_USERNAME",
  pwd: "$MONGO_INITDB_ROOT_PASSWORD",
  roles: [
    {
      role: "readWrite",
      db: "$MONGO_DB_NAME",
    },
  ],
});

db.TaxBand.insertMany([
  { Rate: 0, UpperBound: 12500, Description: "Personal Allowance" },
  { Rate: 20, UpperBound: 50000, Description: "Basic Rate" },
  { Rate: 40, UpperBound: 150000, Description: "Higher Rate" },
  { Rate: 45, Description: "Additional Rate" },
]);
EOF