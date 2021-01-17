# BlazorStaticWebSite
Sample for generate a static WebSite from blazor components. If you have to serve the first html page very fast, you can declare you components for static rendering in a file in the wwwwroot directory.

# Setup Blazor Project

1. Prepare the Blazor solution für WebAssembly Prerendering see the post from John (@jonhilt) at https://jonhilton.net/blazor-wasm-prerendering/
2. Register my StaticWebSite middleware for serve a file from serverside wwwroot path, if the index.html file exists.
```
 app.UseMiddleware<StaticWebSiteMiddleware>();
```
3. Declare the Blazor Components for static rendering
```
@page "/"
@inject HttpClient http
@attribute [GenerateStaticPage()]

<h1>Static WebSite Generator</h1>

<p>
    Welcome to your static website generator
</p>
```
4. Generate on demand the static files with a own component like this:
```
@page "/Generate"
@inject HttpClient http

<h3>Generate</h3>

<input type="button" value="Generate static Website into wwwroot" @onclick="GenerateWebSiteAsync" />

@if (!String.IsNullOrEmpty(state))
{
    <p>@state</p>
}

@code{

    private string state = "";

    private async void GenerateWebSiteAsync()
    {
        state = "generating...";
        StateHasChanged();

        state = await http.GetStringAsync("/WebSiteGenerator/Generate");
        StateHasChanged();
    }
}
```

# Test the static files
After the static pages are generate in the wwwroot directory you can press f5 for refresh the blazor app and now the pages are served static very fast:

![Screenshot](FirstLoad.png)

If you click on FetchData (that is a page without the GenerateStaticPage attribute) the blazor framework loaded and now you are a webassembly application:

![Screenshot](FetchDataLoad.png)
