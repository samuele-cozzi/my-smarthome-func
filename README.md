[![Deploy Azure Function .NET Code](https://github.com/samuele-cozzi/my-smarthome-func/actions/workflows/deploy-functions.yml/badge.svg)](https://github.com/samuele-cozzi/my-smarthome-func/actions/workflows/deploy-functions.yml)

# my-smarthome-func

## Codespaces

### Prerequisites

#### Visual Stido Code Azure Function extension

#### CLI Azure Functions Extension

```bash
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-$(lsb_release -cs)-prod $(lsb_release -cs) main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get update
sudo apt-get install azure-functions-core-tools-4
```

### Create Project with Http Trigger Health Check

### Debug Locally

Debug -> "Attach to .NET Framework"

## Github Action



## TODO

- [ ] Trigger IotHub Function
- [ ] Function settings
- [ ] Functions Add Dapr
- [ ] Business Logic of IotHub Function
- [ ] Trigger Http API Home
- [ ] CloudToDevice message
- [ ] Trigger Http API Home Configuration