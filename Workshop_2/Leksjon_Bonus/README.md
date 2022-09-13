# Bonus leksjon
Her en noen ekstra-oppgaver dersom du skulle bli raskt ferdig.

## Tags 
Forsøk å tagge alle ressurser i med App = Workshop2. 
Etter at det er blitt deployet, kan du gå i portalen og forsøke å finne alle ressurser som du har gitt med den taggen.

## Slot-deployment i produksjon

Slots er en feature i Web Apps som gjør at du kan deploye til en annen slot for å teste den nye versjonen din, for så å swappe denne inn når du har fått testet denne.

Se om du kan få til å sette opp dette i produksjon. Da må du både oppdatere ARM-templates slik at en slot oppdateres, og i tillegg oppdatere deployment-steget av koden i produksjon til å deploye til en slot i stedet for hoved-websiten.

## Azure Rbac For tilgang til Keyvault for Managed identity. 

Managed Identity ved bruk av Rbac er en "passordløs" måte å koble seg på forskjellige azure tjenester. F.eks KeyVault, CosmosDB, Storage account ++ 
Se om du får til å gi Webappen din sin managed identity en Azure AD rolle i scope av ressursgruppen slik at webappen benytter seg av den tilgangen istedengfor access policies. 

For å gjøre det enklere å komme igang, så er dette stegene du må gjøre. 

1.  Finne en rolle som gir nok rettigheter ( For enkelthetsskyld: kan du bruke: Key Vault Administrator ) og definer den som en variabel. 
   f.eks  `"keyVaultAdminUserRoleDefinitionId": "[subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '00482a5a-887f-4fb3-b363-3b7fe8e74483')]"`

2.  I Arm konfig, for key vault `Microsoft.KeyVault/vaults` sett enableRbacAuthorization: false -> true. Denne endringer gjør slik at alt av tilganger til secrets, keys osv må autorisere via Azure AD.
3.  Fjern `Microsoft.KeyVault/vaults/accessPolicies`
4.  Definere en role assignment som vi kan legge inn under resources: 
```
{
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2020-10-01",
      "name": "role-assignment-api-keyVaultAdmin",
      "dependsOn": [
        "[resourceId('Microsoft.Web/sites', variables('apiSiteName'))]"
      ],
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "roleAssignmentName": {
            "value": "[reference(variables('websiteNameWithEnvironment'),'2018-02-01', 'Full').identity.principalId, variables('keyVaultAdminUserRoleDefinitionId'), resourceGroup().id)]"
          },
          "roleDefinitionId": {
            "value": "[variables('keyVaultAdminUserRoleDefinitionId')]"
          },
          "principalId": {
            "value": "[reference(variables('websiteNameWithEnvironment'),'2018-02-01', 'Full').identity.principalId]"
          }
        },
        "template": {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "parameters": {
            "roleAssignmentName": {
              "type": "string" 
            },
            "roleDefinitionId": {
              "type": "string"
            },
            "principalId": {
              "type": "string"
            }
          },
          "variables": {},
          "resources": [
            {
              "type": "Microsoft.Authorization/roleAssignments",
              "apiVersion": "2020-10-01-preview",
              "name": "[parameters('roleAssignmentName')]",
              "properties": {
                "roleDefinitionId": "[parameters('roleDefinitionId')]",
                "principalId": "[parameters('principalId')]"
              }
            }
          ]
        }
      }
    }

```
Commit, og deploy, I portalen kan du sjekke under Identity for webappen din, hvilke tilganger identitien har. 

Merk: Det foregår en del caching når det kommer til managed identities, slik at det kan ta en god stund før du får tilgang til KeyVault, Så ett tips er å eventuelt slette AppServicen din, og redeploye arm templaten. 