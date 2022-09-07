global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Text.Json;
global using System.Threading;
global using System.Threading.Tasks;

global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System.Net.Http;
global using System.Net.Http.Headers;

global using Dapr;
global using Dapr.AzureFunctions.Extension;

global using home.api.models;
global using home.api.config;
global using home.api.services;
global using home.api.services.interfaces;