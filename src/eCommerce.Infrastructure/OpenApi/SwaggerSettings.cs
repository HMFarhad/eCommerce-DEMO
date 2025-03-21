﻿namespace eCommerce.Infrastructure.OpenApi;

#nullable disable
public class SwaggerSettings
{
    public bool Enable { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactUrl { get; set; }
}