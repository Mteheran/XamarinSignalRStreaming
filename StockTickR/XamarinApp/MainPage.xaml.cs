using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace XamarinApp
{
  

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Stock> ItemList { get; set; }

        public MainPage()
        {
            InitializeComponent();

            ItemList = new ObservableCollection<Stock>();

            lstItems.ItemsSource = ItemList;
        }

        async void OnButtonClicked(object sender, EventArgs args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://10.0.2.2:5000/stocks")
                .Build();

            await connection.StartAsync();

            var channel = await connection.StreamAsChannelAsync<Stock>("StreamStocks", CancellationToken.None);
            while (await channel.WaitToReadAsync())
            {
                while (channel.TryRead(out var stock))
                {
                    var CurrentStock = ItemList.FirstOrDefault(p => p.Symbol == stock.Symbol);

                    if(CurrentStock?.Price != stock.Price || CurrentStock==null)
                    {
                        ItemList.Remove(ItemList.FirstOrDefault(p => p.Symbol == stock.Symbol));
                        ItemList.Add(stock);
                    }
                }
            }
        }

    }


    public class Stock
    {
        public string Symbol { get; set; }

        public decimal DayOpen { get; private set; }

        public decimal DayLow { get; private set; }

        public decimal DayHigh { get; private set; }

        public decimal LastChange { get; private set; }

        public decimal Change { get; set; }

        public double PercentChange { get; set; }

        public decimal Price { get; set; }
    }
}
