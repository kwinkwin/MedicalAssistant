# Update DB:
- Update DefaultConnection trong file \MedicalAssistant.API\appsettings.json
- Sau do chay lenh: dotnet ef database update --project "MedicalAssistant.Infrastructure" --startup-project "MedicalAssistant.API"
