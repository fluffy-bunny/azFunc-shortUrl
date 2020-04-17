APP_FRIENDLY_NAME="organics"
LOCATION="eastus2"
RESOURCE_GROUP_NAME="rg-$APP_FRIENDLY_NAME-openhack"


echo "==== Creating Resource Group: $RESOURCE_GROUP_NAME in Location: $LOCATION"
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION