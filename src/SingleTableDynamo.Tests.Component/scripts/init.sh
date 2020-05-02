#!/bin/bash
cd ./app/RacingFormAggregator.Processor
echo "Step executed from: ${PWD}"
dotnet publish -c Release -o publish
cd ./publish
echo "Step executed from: ${PWD}"
zip -r RacingFormAggregator.Processor.zip .
# create lambda function
aws --endpoint-url=http://localstack:4574 lambda create-function --function-name SQSTest --region us-east-1 --runtime dotnetcore2.1 --handler RacingFormAggregator.Processor::RacingFormAggregator.Processor.Function::FunctionHandler --zip-file fileb://RacingFormAggregator.Processor.zip --role arn:aws:iam:awslocal --environment Variables="{ASPNETCORE_ENVIRONMENT=Container,AWS_ACCESS_KEY_ID=dummyaccess,AWS_SECRET_ACCESS_KEY=dummysecret,AWS_DEFAULT_REGION=us-east-1}"
# create sqs queue
aws --endpoint-url=http://localstack:4576 sqs create-queue --queue-name test_queue --region us-east-1
# create a mapping between lambda and an event source (in this case the above queue)
aws --endpoint-url=http://localstack:4574 lambda create-event-source-mapping --function-name SQSTest --event-source-arn arn:aws:sqs:us-east-1:000000000000:test_queue
# create dynamoDB table
aws --endpoint-url=http://localstack:4569 dynamodb create-table --table-name test_db --attribute-definitions AttributeName=HashKey,AttributeType=S AttributeName=SortKey,AttributeType=S --key-schema AttributeName=HashKey,KeyType=HASH AttributeName=SortKey,KeyType=RANGE --billing-mode PAY_PER_REQUEST

echo "App is ready"