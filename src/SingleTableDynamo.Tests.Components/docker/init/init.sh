#!/bin/bash

# create dynamoDB table
echo "Setup test_db dynamodb"
aws --endpoint-url=http://localstack:4569 dynamodb create-table --table-name test_db --attribute-definitions AttributeName=HashKey,AttributeType=S AttributeName=SortKey,AttributeType=S --key-schema AttributeName=HashKey,KeyType=HASH AttributeName=SortKey,KeyType=RANGE --billing-mode PAY_PER_REQUEST
echo "App is ready"
