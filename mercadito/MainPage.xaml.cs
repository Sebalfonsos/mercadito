using CommunityToolkit.Maui.Alerts;
using mercadito.Models;
using Microsoft.Maui.ApplicationModel.Communication;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static mercadito.Models.Productos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace mercadito
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private HttpClient _httpClient;

        CommunityToolkit.Maui.Core.IToast toast = Toast.Make("Wrong Email or Password", CommunityToolkit.Maui.Core.ToastDuration.Short,30);

        public MainPage()
        {


            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(EnviromentVariables.apiBaseURL);
          

            //getInfo();

           
        }

        public async void getInfo()
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
                }

            }
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {

            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            var parametros = new StringContent($"{{ \"email\": \"{email}\", \"password\": \"{password}\" }}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("/api/user/login", parametros);

            Console.WriteLine((int)response.StatusCode);

            if ((int)response.StatusCode == 200)
            {
                string content = await response.Content.ReadAsStringAsync();

                UserLogged.ApiResponse respuesta = JsonSerializer.Deserialize<UserLogged.ApiResponse>(content);


                EnviromentVariables.APITOKEN = respuesta.token;
                EmailEntry.IsEnabled = false;
                PasswordEntry.IsEnabled = false;

                EmailEntry.IsEnabled = true;
                PasswordEntry.IsEnabled = true;

                Navigation.PushAsync(new ViewProducts());
            }
            else
            {

                toast.Show();
            }
        }
        private async void Register_Clicked(object sender, EventArgs e)
        {

           
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;
            string nombre = null;
            if(email != null && email != "" && password != null && password != "") {
                 nombre = await DisplayPromptAsync("Escribe tu nombre", "", "Crear", "Cancelar", maxLength: 50, keyboard: Keyboard.Text);

            }


            if (nombre != null && nombre != "" )
            {

                var parametros = new StringContent($"{{ \"email\": \"{email}\", \"password\": \"{password}\", \"name\": \"{nombre}\" }}", Encoding.UTF8, "application/json");


                HttpResponseMessage response = await _httpClient.PostAsync("/api/user/create", parametros);

                Console.WriteLine((int)response.StatusCode);

                if ((int)response.StatusCode == 200)
                {

                    CommunityToolkit.Maui.Core.IToast toastUserCreated = Toast.Make("User Created", CommunityToolkit.Maui.Core.ToastDuration.Short, 30);

                    toastUserCreated.Show();

                }
                else
                {
                    CommunityToolkit.Maui.Core.IToast toastUserCreatedError = Toast.Make("Error Creating user (maybe already exists)", CommunityToolkit.Maui.Core.ToastDuration.Short, 15);

                    toastUserCreatedError.Show();
                }
            }

        }
    }

}
