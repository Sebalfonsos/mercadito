﻿namespace mercadito
{
    public partial class App : Application
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();
            MainPage = mainPage;
            //MainPage = new AppShell();
            //MainPage = new ViewProducts();
        }
    }
}
