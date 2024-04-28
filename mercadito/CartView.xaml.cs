using mercadito.LocalDbModels;


namespace mercadito;

public partial class CartView : ContentPage
{
    private readonly LocalDbService _dbService;
    private int _editItemId;
    
    public CartView(LocalDbService dbService)
	{
		InitializeComponent();
        _dbService = dbService;
        Task.Run(async () => listView.ItemsSource = await _dbService.getCart());
	}

    private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var item = (Cart)e.Item;
        var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

        switch (action)
        {
            case "Delete":
                await _dbService.delete(item);
                listView.ItemsSource = await _dbService.getCart();
                break;


        }
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        if(_editItemId == 0)
        {
            //await _dbService.create(new Cart
            //{
            //    Nombre = nombreEntry.Text,
            //    Descripcion = descripcionEntry.Text,
            //    Imagen = imagenEntry.Text,
            //    Precio = int.Parse(precioEntry.Text),
            //    Cantidad = int.Parse(cantidadEntry.Text)

            //});
        }

        listView.ItemsSource = await _dbService.getCart();

    }
}