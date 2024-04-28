using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using mercadito.LocalDbModels;
using mercadito.Models;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls.Shapes;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using static mercadito.Models.Comprar;
using static mercadito.Models.Productos;

namespace mercadito;

public partial class ViewProducts : ContentPage
{

    private HttpClient _httpClient;
    private readonly LocalDbService _dbService;
    public ViewProducts(LocalDbService dbService)
	{
		InitializeComponent();
        _dbService = dbService;
        //LABELCITO.Text = EnviromentVariables.APITOKEN;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(EnviromentVariables.apiBaseURL);

        string bearerToken = Preferences.Get("Token", string.Empty);
        // Establecer el token de portador en el encabezado de autorización
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        // Establecer el encabezado 'Accept' en la solicitud
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        generateProducts();

       
        ICommand refreshCommand = new Command(async () =>
        {
            productStackLayout.Children.Clear();
            await generateProducts();
            refreshView.IsRefreshing = false;
        });
        refreshView.Command = refreshCommand;
    }


    public async Task generateProducts()
    {

        HttpResponseMessage response = await _httpClient.GetAsync("/api/productos");
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();


            List<Productos.Class1> productos = JsonSerializer.Deserialize<List<Productos.Class1>>(content);




            foreach (var producto in productos)
            {
                Console.WriteLine($"ID: {producto.id}");
                Console.WriteLine($"Nombre: {producto.nombre}");
                Console.WriteLine($"Descripción: {producto.descripcion}");
                Console.WriteLine($"Imagen: {producto.imagen}");
                Console.WriteLine($"Precio: {producto.precio}");
                Console.WriteLine($"Cantidad: {producto.cantidad}");
                Console.WriteLine($"Creado en: {producto.created_at}");
                Console.WriteLine($"Actualizado en: {producto.updated_at}");
                Console.WriteLine();

                if (producto.cantidad == 0)
                {
                    continue;
                }


                var image = new Image
                {
                    Source = EnviromentVariables.apiBaseURL + producto.imagen,
                    Margin = new Thickness(10),
                    WidthRequest = 120,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };

                var nameLabel = new Label
                {
                    Text = producto.nombre,
                    TextColor = Colors.Black,
                    FontSize = 18,
                    WidthRequest = 200,
                    Margin = new Thickness(0, 10, 5, 0),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                var descripcionLabel = new Label
                {
                    Text = producto.descripcion,
                    TextColor = Colors.Black,
                    FontSize = 12.5,
                    WidthRequest = 200,
                    Margin = new Thickness(0, 10, 5, 0),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                var priceLabel = new Label
                {
                    Text = "$ "+producto.precio,
                    TextColor = Colors.Black,
                    FontSize = 17,
                    WidthRequest = 200,
                    Margin = new Thickness(0, 5, 5, 0),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                var buyButton = new Button
                {
                    Text = "Buy",
                    BackgroundColor = Color.FromHex("#4CAF50"), // Color verde (puedes ajustar este color según tus preferencias)
                    TextColor = Colors.White,
                    WidthRequest = 100,
                    HeightRequest = 40,
                    Margin = new Thickness(10, 0, 0, 10), // Ajusta los márgenes según sea necesario
                    HorizontalOptions = LayoutOptions.Start // Alinea el botón a la izquierda
                };

                

                buyButton.Clicked += async (sender, e) =>
                {
                    // Aquí puedes agregar la lógica para procesar la compra
                    //await DisplayAlert("Compra", "Has comprado " + producto.nombre, "Aceptar");
                    string result = await DisplayPromptAsync("Seleccionado: "+producto.nombre,"Cantidad disponible: "+ producto.cantidad, "Comprar", "Cancelar", "Cantidad a comprar", maxLength: 10, keyboard: Keyboard.Numeric);
                   

                    
                    if (result != "" && result != null)
                    {
                        
                        // Haz algo con el valor ingresado
                        var statusCode = await comprar(producto.id, int.Parse(result));

                        if (statusCode == 201)
                        {
                            producto.cantidad = producto.cantidad - int.Parse(result);
                        }
                        
                    }

                };

                var addToCartButton = new Button
                {
                    Text = "Add to cart",
                    BackgroundColor = Color.FromHex("#4CAF50"), // Color verde (puedes ajustar este color según tus preferencias)
                    TextColor = Colors.White,
                    WidthRequest = 100,
                    HeightRequest = 40,
                    Margin = new Thickness(10, 0, 0, 10), // Ajusta los márgenes según sea necesario
                    HorizontalOptions = LayoutOptions.Start // Alinea el botón a la izquierda
                };

                addToCartButton.Clicked += async (sender, e) =>
                {
                    // Aquí puedes agregar la lógica para procesar la compra
                    //await DisplayAlert("Compra", "Has comprado " + producto.nombre, "Aceptar");
                    string result = await DisplayPromptAsync("Seleccionado: " + producto.nombre, "Cantidad disponible: " + producto.cantidad, "Comprar", "Cancelar", "Cantidad a comprar", maxLength: 10, keyboard: Keyboard.Numeric);



                    if (result != "" && result != null)
                    {

                        // Haz algo con el valor ingresado
                        await _dbService.create(new Cart
                        {
                            Nombre = producto.nombre,
                            Descripcion = producto.descripcion,
                            Imagen = EnviromentVariables.apiBaseURL + producto.imagen,
                            Precio = producto.precio ,
                            Cantidad = int.Parse(result)
                        });

                        IToast toastAdd = Toast.Make("Añadido al carrito", ToastDuration.Long, 20);
                        toastAdd.Show();

                    }

                };
                var separator = new Rectangle
                {
                    Fill = Color.FromHex("#ededed"),
                    HeightRequest = 2,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };


                var horizontalLayout = new HorizontalStackLayout();
                var verticalLayout = new VerticalStackLayout();

                // Agregar los elementos al VerticalStackLayout
                horizontalLayout.Children.Add(image);
                verticalLayout.Children.Add(nameLabel);
                verticalLayout.Children.Add(descripcionLabel);
                verticalLayout.Children.Add(priceLabel);
                verticalLayout.Children.Add(buyButton);
                verticalLayout.Children.Add(addToCartButton);
                horizontalLayout.Children.Add(verticalLayout);

                productStackLayout.Children.Add(horizontalLayout);

                productStackLayout.Children.Add(separator); // Agrega el separador

            }

        }

        
    }

    //private void actualizar_Clicked(object sender, EventArgs e)
    //{
    //    productStackLayout.Children.Clear();
    //    generateProducts();
    //}

 
    private async Task<int> comprar(int id, int cantidad)
    {

       // string bearerToken = EnviromentVariables.APITOKEN; // Suponiendo que tienes una clase llamada EnviromentVariables con una propiedad APITOKEN
       
        var parametrosObject = new
        {
            producto_id = id,
            cantidad = cantidad
        };

        var Parametrosjson = JsonSerializer.Serialize(parametrosObject);


        var parametros = new StringContent(Parametrosjson, Encoding.UTF8, "application/json");
        

        // Enviar la solicitud POST con el contenido y el encabezado de autorización
        HttpResponseMessage response = await _httpClient.PostAsync("/api/compraproductos", parametros);

        // Leer el contenido de la respuesta
        string content = await response.Content.ReadAsStringAsync();
        // Deserializar el contenido de la respuesta en un objeto Comprar.Compra
        Comprar.Compra compra = JsonSerializer.Deserialize<Comprar.Compra>(content);

        // Mostrar un mensaje de éxito usando CommunityToolkit.Maui.Core.IToast
        IToast toast = Toast.Make(compra.message, ToastDuration.Long, 20);
        toast.Show();


        return (int)response.StatusCode;
    }

    private async void verCarrito_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CartView(_dbService));
    }

    private async void closeSesion_Clicked(object sender, EventArgs e)
    {
        HttpResponseMessage response = await _httpClient.GetAsync("/api/validateToken");

        if((int)response.StatusCode == 200)
        {
            HttpResponseMessage closeresponse = await _httpClient.GetAsync("/api/user/logout");
            if((int)closeresponse.StatusCode == 200)
            {
                var mainPage = new MainPage(_dbService);
                Application.Current.MainPage = mainPage;
            }
        }

    }
}