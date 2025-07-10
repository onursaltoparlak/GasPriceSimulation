using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace Project15_GasPriceSimulation
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    double dieselPrice;
    double gasolinePrice;
    double lpgPrice;
    double gasAmount = 0;
    double currentAmount = 0;
    double totalPrice = 0;
    double pricePerLiter = 0;
    double visualPrice = 0;
    int count = 0;

    private async void Form1_Load(object sender, EventArgs e)
    {
      var client = new HttpClient();
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = new Uri("https://gas-price.p.rapidapi.com/europeanCountries"),
        Headers =
                {
                    { "x-rapidapi-key", "a8c893afc7msh7f7ff35639d05acp1bc012jsn185c40f41dc2" },
                    { "x-rapidapi-host", "gas-price.p.rapidapi.com" },
                },
      };
      using (var response = await client.SendAsync(request))
      {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(body);

        
        var gasolineJsonValue = json["results"][42]["gasoline"];
        var dieselJsonValue = json["results"][42]["diesel"];
        var lpgJsonValue = json["results"][42]["lpg"];

        gasolinePrice = (Convert.ToDouble(gasolineJsonValue) * 47.15) / 1000;
        dieselPrice = (Convert.ToDouble(dieselJsonValue) * 47.15) / 1000;
        lpgPrice = (Convert.ToDouble(lpgJsonValue) * 47.15) / 1000;

        txtGasolinePrice.Text = gasolinePrice.ToString("0.00") + " ₺";
        txtDieselPrice.Text = dieselPrice.ToString("0.00") + " ₺";
        txtLpgPrice.Text = lpgPrice.ToString("0.00") + " ₺";

        Console.WriteLine(body);
      }
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      if (!double.TryParse(txtGasAmount.Text, out gasAmount) || gasAmount <= 0)
      {
        MessageBox.Show("Lütfen geçerli bir litre değeri giriniz.");
        return;
      }

      if (rdbGasoline.Checked)
      {
        pricePerLiter = gasolinePrice;
      }
      else if (rdbDiesel.Checked)
      {
        pricePerLiter = dieselPrice;
      }
      else if (rdbLPG.Checked)
      {
        pricePerLiter = lpgPrice;
      }
      else
      {
        MessageBox.Show("Lütfen bir yakıt türü seçiniz.");
        return;
      }

      totalPrice = gasAmount * pricePerLiter;

      progressBar1.Value = 0;
      progressBar1.Maximum = (int)gasAmount;

      currentAmount = 0;
      visualPrice = 0;
      count = 0;
      txtTotalPrice.Text = "0.00 ₺";
      timer1.Interval = 100;
      timer1.Start();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (count < gasAmount)
      {
        count++;
        currentAmount += 1;
        visualPrice = currentAmount * pricePerLiter;

        txtTotalPrice.Text = visualPrice.ToString("0.00") + " ₺";
        progressBar1.Value = (int)currentAmount;
      }
      else
      {
        timer1.Stop();
        txtTotalPrice.Text = totalPrice.ToString("0.00") + " ₺";
      }

    }
  }
}
