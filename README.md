# tax-calculator

# Objective
This project aims to create a simple AWS lambda function to calculate the amount of income tax according to a list of income tax band provided from an external mongo database.

# Prerequisite
- AWS SAM: This project was based on the default template of AWS SAM, threfore you need to install AMS SAM to run it. refer to [this](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html) for installation detaiils. For detail about AWS SAM, refer to [original AWS SAM Readme](/README.SAM.md) file which constains rich information about AWS SAM.

- Docker: docker compose is used to run the local mongo database, you need it if you want to test the project locally. Refer to [this](https://docs.docker.com/get-docker/) to install docker.

- Environment variables: This project rely on environment variables to store it's config and secrets, you MUST have 
the environment variables in below to run the project properly.

```
MONGO_INITDB_ROOT_USERNAME={User name for the setting up the local mongo database}
MONGO_INITDB_ROOT_PASSWORD={password for the setting up the local mongo database}
MONGO_DB_CONNECTION_STR={mongo db connection string}
MONGO_DB_NAME={the mongo db database name}
```

Exmaple
```š
MONGO_INITDB_ROOT_USERNAME=taxAdmin
MONGO_INITDB_ROOT_PASSWORD=topsecret
MONGO_DB_CONNECTION_STR=mongodb://txtAdmin:topsecret@192.168.1.123:27017/taxCalculator
MONGO_DB_NAME=taxCalculator
```
# To run the project locally

1. Make sure you have setup the environment variables in prerequisite section
2. Navigate to middleware folder, execute the following command to spin up the mongo db
```console
docker compose up
```
3. Navigate to the top level of the project, execute the following command
```console
sam build && sam local start-api
```
4. Navigate to http://localhost:3000/tax/{amount} to get the tax of the amount provided
   ## Example: 
   http://localhost:3000/tax/52000.00

5. Append detail flag http://localhost:3000/tax/{amount}?detail to get tax and details of the amount provided
   ## Example: 
   http://localhost:3000/tax/52000.00?detail

# To run Tests

1. Navigate to test/TaxCalculator.Test 
2. The tests include both integration and unit tests, therefore you need to setup the environment variables in prerequisite to run all tests properly.
2. Execute the following command
```console
dotnet test
```

# Remarks

- tax bands is inserted to the mongo database during init as shown below
```Json
[
  { Rate: 0, UpperBound: 12500, Description: "Personal Allowance" },
  { Rate: 20, UpperBound: 50000, Description: "Basic Rate" },
  { Rate: 40, UpperBound: 150000, Description: "Higher Rate" },
  { Rate: 45, Description: "Additional Rate" },
]
```
- if you want to modify them, navigate to http://localhost:8081 and login with the username and password you set in the environment variables in prerequisite section.

- if somehow you mess up the data, navigate to the middleware folder and issue the following command
```shell
docker compose down
docker volume remove middleware_mongo-data
```
