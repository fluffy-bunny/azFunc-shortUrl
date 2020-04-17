![fluffy-bunny-banner](https://raw.githubusercontent.com/fluffy-bunny/static-assets/master/fluffy-bunny-banner.png)  
# azFunc-url-shortener

## Initialize Terraform for Cloud State

```bash
cd az-setup/
./
./000.step-one-setup-terraform.sh 
```

This will create a separate resource group for only terraform state management.
```bash
{
    "resource_group":{
        "name":"rg-terraform-shorturl",
        "resources":[
            {
                "type":"storage account",
                "name":"stterraformshorturl"
            },
            {
                "type":"key vault",
                "name":"kv-terraform-shorturl"
            }
        ]
    }
}
```

Now you can do terraform only stuff moving foward.

**make sure you have Terraform v0.12.20**  
[how-to-install-terraform-centos-ubuntu](https://phoenixnap.com/kb/how-to-install-terraform-centos-ubuntu)  
[terraform download](https://www.terraform.io/downloads.html)

Terraform doesn't like dependencies that it needs to create to then be injected into other resources.
So it allows you to target a resource.
In this case our azure function's principal_id needs to be injected into a keyvault policy.  So the function needs to exist first.

```bash
cd az-setup/terraform/002.main-infrastructure/
terraform init
terraform plan -target=azurerm_function_app.azfunc_shorturl -out=tf.plan    
terraform apply tf.plan                                                                                                                        
```

## Key Vault
Terraform can write secrets to key vault, but to do this it is under an account.
1. whomever runs terraform apply needs to be able to create resources
2. You need to put that principal into the keyvault's access_policy so that we can then write to the vault.
```
module "key_vault" {
  source = "innovationnorway/key-vault/azurerm"

  name = var.az_keyvault_name

  resource_group_name = azurerm_resource_group.rg.name

  access_policies = [
      {
        user_principal_names = ["hstahl@symantec.com"]
        secret_permissions   = ["backup","delete","get", "list","purge","recover","restore","set"]
      },
      {
        object_ids        = ["${azurerm_function_app.azfunc_shorturl.identity.0.principal_id}"]
        secret_permissions   = ["get"]
      },
      
  ]


  secrets = {
    "message" = "Hello, world!"
  }
}
```
Here I added myself **hstahl@symantec.com** as that principal.  Obviously this will never fly in production and that principal will be some trusted SRE.  

 Ok so now our azure function exists so we can do pass 2
 ```bash
cd az-setup/terraform/002.main-infrastructure/
terraform init
terraform plan -out=tf.plan    
terraform apply tf.plan                                                                                                                        
```
