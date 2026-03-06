// --- Microsoft Frameworks & Identity ---
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;

// --- Third-Party Libraries ---
global using StackExchange.Redis;
global using System.Text;

// ---  Onion Layers (Infrastructure/Persistence) ---
global using ECommerce.Presistance.Data.DBContexts;
global using ECommerce.Presistance.Data.DataSeeding;
global using ECommerce.Presistance.IdentityData.DbContext;
global using ECommerce.Presistance.IdentityData.DataSeed;
global using ECommerce.Presistance.Repository;

// ---  Onion Layers (Application/Domain) ---
global using ECommerce.Domain.Contracts;
global using ECommerce.Domain.Entities.IdentityModule;
global using ECommerce.Service;
global using ECommerce.ServiceAbstraction;

// --- Web Specific (Extensions & Middlewares) ---
global using ECommerceWeb.CustomeMiddleWare;
global using ECommerceWeb.Extentions;
global using ECommerceWeb.Factory;