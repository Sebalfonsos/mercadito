using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using mercadito.LocalDbModels;
using mercadito.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static mercadito.Models.Productos;


namespace mercadito;

public partial class CartView : ContentPage
{
    private readonly LocalDbService _dbService;
    private int _editItemId;
    private HttpClient _httpClient;

    public CartView(LocalDbService dbService)
	{
		InitializeComponent();
        _dbService = dbService;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(EnviromentVariables.apiBaseURL);

        string bearerToken = Preferences.Get("Token", string.Empty);
        // Establecer el token de portador en el encabezado de autorización
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        // Establecer el encabezado 'Accept' en la solicitud
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        Task.Run(async () => collectionView.ItemsSource = await _dbService.getCart());
	}

   

  

    private async void cleanCartButton_Clicked(object sender, EventArgs e)
    {
        await _dbService.clearCart();
        collectionView.ItemsSource = await _dbService.getCart();
    }

    
   
    private async void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            var item = (Cart)e.CurrentSelection[0];
            var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

            switch (action)
            {
                case "Delete":
                    await _dbService.delete(item);
                    collectionView.ItemsSource = await _dbService.getCart();
                    break;
                case "Edit":
                    string result = await DisplayPromptAsync("Seleccionado: " + item.Nombre, "Cantidad actual: " + item.Cantidad, "Comprar", "Cancelar", "Nueva cantidad a comprar", maxLength: 10, keyboard: Keyboard.Numeric);
                    
                    await _dbService.update(new Cart
                    {
                        Id = item.Id,
                        productId = item.productId,
                        Nombre = item.Nombre,
                        Descripcion = item.Descripcion,
                        Imagen = item.Imagen,
                        Precio = item.Precio,
                        Cantidad = int.Parse(result)


                    });
                    collectionView.ItemsSource = await _dbService.getCart();

                    break;
            }
            collectionView.SelectedItem = null;
        }
        

    }

    private async void buyCartButton_Clicked(object sender, EventArgs e)
    {
        var cart = await _dbService.getCart();

        var listaProductos = new List<object>();

        foreach (var item in cart)
        {
            listaProductos.Add(new { ProductoId = item.productId, Cantidad = item.Cantidad });
        }

        var parametrosObject = new
        {
            productos = listaProductos
        };

        var Parametrosjson = JsonSerializer.Serialize(parametrosObject);

        Console.WriteLine(Parametrosjson);
        var parametros = new StringContent(Parametrosjson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync("/api/compraproductos", parametros);

        // Leer el contenido de la respuesta
        string content = await response.Content.ReadAsStringAsync();
        // Deserializar el contenido de la respuesta en un objeto Comprar.Compra
        Comprar.Compra compra = JsonSerializer.Deserialize<Comprar.Compra>(content);

        // Mostrar un mensaje de éxito usando CommunityToolkit.Maui.Core.IToast
        IToast toast = Toast.Make(compra.message, ToastDuration.Long, 20);
        toast.Show();


        foreach (var item in compra.productos_comprados)
        {

             Cart selected = await _dbService.getByproductId(item.ProductoId);

             await _dbService.delete(selected);
        }

        collectionView.ItemsSource = await _dbService.getCart();

    }
}