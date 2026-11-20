#region System
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Expressions;
#endregion

global using TradingDashboard.Application.Common.Models;
global using TradingDashboard.Application.Features.CotReports.Queries.GetCotAvailableDates;
global using TradingDashboard.Application.Features.CotReports.Queries.GetCotLatestReport;
global using TradingDashboard.Application.Features.CotReports.Queries.GetCotReportSeries;
global using TradingDashboard.Application.Repository.Cot;
global using TradingDashboard.Domain.Entities;
global using TradingDashboard.Domain.Enums;
global using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;
global using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
global using Xunit;
