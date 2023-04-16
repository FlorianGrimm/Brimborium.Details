global using global::System;
global using global::System.Collections.Generic;
global using global::System.IO;
global using global::System.Linq;
global using global::System.Net.Http;
global using global::System.Text;
global using global::System.Threading;
global using global::System.Threading.Tasks;
global using global::System.Text.Json;
global using global::System.Text.Json.Serialization;
global using global::System.Text.RegularExpressions;

global using global::Markdig;
global using global::Markdig.Helpers;
global using global::Markdig.Parsers;
global using global::Markdig.Renderers;
global using global::Markdig.Renderers.Normalize;
global using global::Markdig.Syntax;
global using global::Markdig.Syntax.Inlines;

global using global::Microsoft.Extensions.Configuration;
global using global::Microsoft.Extensions.DependencyInjection;
global using global::Microsoft.Extensions.Options;
global using global::Microsoft.Extensions.Hosting;
global using global::Microsoft.Extensions.FileProviders;
global using global::Microsoft.AspNetCore.Builder;
global using global::Microsoft.AspNetCore.SpaServices;

global using global::Microsoft.CodeAnalysis.MSBuild;
global using global::Microsoft.CodeAnalysis.CSharp;
global using global::Microsoft.CodeAnalysis.CSharp.Syntax;
global using global::Microsoft.CodeAnalysis;
global using global::Microsoft.CodeAnalysis.FindSymbols;

global using global::Brimborium.Details.Controller;
global using global::Brimborium.Details.Service;

global using global::Brimborium.Details;
global using global::Brimborium.Details.Cfg;
global using global::Brimborium.Details.Enhancement;
global using global::Brimborium.Details.Parse;
global using global::Brimborium.Details.Repository;
global using global::Brimborium.Details.Utility;
global using global::Brimborium.Details.Watch;
