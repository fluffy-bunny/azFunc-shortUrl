TOPIC_NAME="tpc-shorturl-tenant-usage"
RESOURCE_GROUP="rg-shorturl"

endpoint=$(az eventgrid topic show --name $TOPIC_NAME -g $RESOURCE_GROUP --query "endpoint" --output tsv)
echo "endpoint: $endpoint"
key=$(az eventgrid topic key list --name $TOPIC_NAME -g $RESOURCE_GROUP --query "key1" --output tsv)
echo "key: $key"

event='[ {"id": "'"$RANDOM"'", "eventType": "recordInserted", "subject": "myapp/vehicles/motorcycles", "eventTime": "'`date +%Y-%m-%dT%H:%M:%S%z`'", "data":{ "make": "Ducati", "model": "Monster"},"dataVersion": "1.0"} ]'
curl -X POST -H "aeg-sas-key: $key" -d "$event" $endpoint