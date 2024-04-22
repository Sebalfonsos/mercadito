using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using mercadito.Models;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls.Shapes;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static mercadito.Models.Productos;

namespace mercadito;

public partial class ViewProducts : ContentPage
{

    private HttpClient _httpClient;
    public ViewProducts()
	{
		InitializeComponent();

        //LABELCITO.Text = EnviromentVariables.APITOKEN;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(EnviromentVariables.apiBaseURL);

        generateProducts();

    }


    public async void generateProducts()
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
                    Text = "Comprar",
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
                horizontalLayout.Children.Add(verticalLayout);

                productStackLayout.Children.Add(horizontalLayout);

                productStackLayout.Children.Add(separator); // Agrega el separador

            }

        }

        
    }

    private void actualizar_Clicked(object sender, EventArgs e)
    {
        productStackLayout.Children.Clear();
        generateProducts();
    }
    private async Task<int> comprar(int id, int cantidad)
    {

        string bearerToken = EnviromentVariables.APITOKEN; // Suponiendo que tienes una clase llamada EnviromentVariables con una propiedad APITOKEN

        var parametros = new StringContent($"{{ \"producto_id\": \"{id}\", \"cantidad\": \"{cantidad}\" }}", Encoding.UTF8, "application/json");

        // Establecer el token de portador en el encabezado de autorización
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        // Establecer el encabezado 'Accept' en la solicitud
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

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
}