using Asf.Taxi.API.Authentication;
using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Extensions;
using Microsoft.AspNetCore.Authentication;
using SmartTech.Common.API;
using SmartTech.Common.Authentication;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Configuration.AzureApp;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAzureAppConfiguration("TaxiDriver");

// Add services to the container.
builder.AddSmartTechServices("TaxiAPI", options =>
{
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
	options.Filters.Add(new AuthorityHeaderActionFilter());
});
builder.Services.AddTaxiServices(builder.Configuration);
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ITaxiUserContext, UserContext>();
builder.Services.AddScoped<IClaimsTransformation, TaxiDriverRolesClaimTransformation>();
SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncFusion:LicenseKey"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSmartTechConfiguration(virtualDirectoryName: "taxidriver");
await app.RunAsync();