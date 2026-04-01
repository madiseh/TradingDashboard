#region System
global using System.Net.Http;
global using System.Text.Json;
#endregion

global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using TradingDashboard.Application.Common.Models;
global using TradingDashboard.Domain.Entities;
global using TradingDashboard.Features.Cot.CollectService.Models;
global using TradingDashboard.Features.Cot.CollectService.Options;
global using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
global using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
