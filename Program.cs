using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ProductRepository;

var builder = WebApplication.CreateBuilder(args);

//criar o serviço para o BD, 
//mudou qd injeta no construtor
// builder.Services.AddDbContext<ApplicationDBContext>();
builder.Services.AddSqlServer<ApplicationDBContext>(builder.Configuration["Database:SqlServer"]);


var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello API!");
app.MapGet("/user", () => "Sou Eu, Debora!");
// app.MapGet("/AddHeader", (HttpResponse response) => response.Headers.Add("Teste", "Debora testando"));
app.MapGet("/AddHeader", (HttpResponse response) => {
    response.Headers.Add("Teste", "Debora testando");
    return new {Name = "Debora Post", Age = 35};
    });
app.MapPost("/", () => new {Name = "Debora Post", Age = 35});

//parametro Body
// app.MapPost("/saveproduct", (Product product) => {
//     return product.Code + "-" + product.Name;
// });
//metodo antes da classe ProductRequest
// 
app.MapPost("/products", (ProductRequest productRequest, ApplicationDBContext context) => {
    // var category  = context.Categories.ToList();  = listar todas as categorias
    var category  = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };
    // ProductRepository.Add(product); //esse era guardado em memoria

    //tags
    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{Name = item});
        }
    }
    context.Products.Add(product); // add no contexto bd
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});

//parametro URL
// url - api.app.com/user?datastart={date}&dateend={date}
// app.MapGet("/product", ([FromQuery] string dataStart, [FromQuery] string dataEnd)=> {
//     return dataStart + "-" + dataEnd;
// });
// rota (parametro é obrigatorio) - api.app.com/user/{code}
// app.MapGet("/getproduct/{code}", ([FromRoute]string code)=> {
//     return code;
// });
app.MapGet("/product/{id}", ([FromRoute]int id, ApplicationDBContext context)=> {
    var product = context.Products
    .Include(p => p.Category) //icluir os relacionamentos
    .Include(p => p.Tags)
    .Where(p => p.Id == id)
    .First();
    // var product = ProductRepository.GetBy(id);
    if(product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

//parametro Header
// app.MapGet("/byheader", (HttpRequest request) => {
//         return request.Headers["producte-code"].ToString();
//     });

//Editando;
app.MapPut("/product/{id}", ([FromRoute]int id,ProductRequest productRequest, ApplicationDBContext context) => {
    // var productSaved = ProductRepository.GetBy(product.Code);
    // productSaved.Name = product.Name;
     var product = context.Products
     //icluir os relacionamentos
    .Include(p => p.Tags)
    .Where(p => p.Id == id).First();
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();

    product.Code = productRequest.Code;
    product.Name = productRequest.Name;
    product.Description = productRequest.Description;
    product.Category = category;
    product.Tags = new List<Tag>();
     if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{Name = item});
        }
    }
    return Results.Ok();
});

//Deletando
app.MapDelete("/product/{id}", ([FromRoute] int id, ApplicationDBContext context) => {
    var product = context.Products
    .Where(p => p.Id == id).First();
    context.Remove(product);
    context.SaveChanges();
    // var productSaved = ProductRepository.GetBy(id);
    // ProductRepository.Remove(productSaved);
    return Results.Ok();
});

//lendo a conexao do BD
app.MapGet("/configuration/database", (IConfiguration configuration) => {
    return Results.Ok(configuration["database:connection"]);
});

app.Run();

//repositorio "local"
public static partial class ProductRepository {
    public static List<Product> Products {get; set;}= Products = new List<Product>();

    //pegando os "BD" dos appsetting
    public static void Init(IConfiguration configuration){
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product){
        Products.Add(product);
    }

    public static Product GetBy(string code){
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product) {
        Products.Remove(product);
    }
}
