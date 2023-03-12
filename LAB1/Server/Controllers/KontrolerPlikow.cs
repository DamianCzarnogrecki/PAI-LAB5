using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using IronXL;

namespace LAB1.Server.Controllers
{
    [Route("api/plik")]
    [ApiController]
    public class KontrolerPlikow : ControllerBase
    {
        private readonly IWebHostEnvironment iwhe;
        private readonly Kontekst kontekst;

        public KontrolerPlikow(IWebHostEnvironment iwhe, Kontekst kontekst)
        {
            this.iwhe = iwhe;
            this.kontekst = kontekst;
        }

        [HttpGet]
        public IActionResult PobranieTabeli()
        {
            using (SqlConnection polaczenie = new SqlConnection(@"Data Source=localhost; Initial Catalog=panstwa; Integrated Security=True"))
            {
                polaczenie.Open();
                using (SqlCommand kwerenda = new SqlCommand("SELECT * FROM dbo.Panstwo", polaczenie))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(kwerenda))
                    {
                        DataSet dane = new DataSet();
                        adapter.Fill(dane);
                        string daneCSV = TabelaDoPliku(dane.Tables[0]);
                        var daneBity = Encoding.UTF8.GetBytes(daneCSV);
                        polaczenie.Close();
                        return File(daneBity, "text/csv", "dane.csv");
                    }
                }
            }
        }

        private string TabelaDoPliku(DataTable tabela)
        {
            StringBuilder stringbuilder = new StringBuilder();
            IEnumerable<string> kolumny = tabela.Columns.Cast<DataColumn>().Select(x => x.ColumnName);
            stringbuilder.AppendLine(string.Join(',', kolumny));
            foreach (DataRow datarow in tabela.Rows)
            {
                IEnumerable<string> wpisy = datarow.ItemArray.Select(x => string.Concat("\"", x.ToString().Replace("\"", "\"\""), "\""));
                stringbuilder.AppendLine(string.Join(',', wpisy));
            }
            return stringbuilder.ToString();
        }

        [HttpPost("NaSerwer")]
        public async Task<ActionResult<List<PlikOdUzytkownika>>> NaSerwer(List<IFormFile> pliki)
        {
            List<PlikOdUzytkownika> plikiOdUzytkownika = new List<PlikOdUzytkownika>();

            foreach (var plik in pliki)
            {
                var plikOdUzytkownika = new PlikOdUzytkownika();
                var oryginalnaNazwa = plik.FileName;
                var sciezka = Path.Combine(iwhe.ContentRootPath, "wrzucone", "plik.csv");

                plikOdUzytkownika.OryginalnaNazwa = oryginalnaNazwa;

                await using FileStream filestream = new(sciezka, FileMode.Create);
                await plik.CopyToAsync(filestream);

                plikOdUzytkownika.NadanaNazwa = "plik.csv";
                plikiOdUzytkownika.Add(plikOdUzytkownika);

                kontekst.Wrzuc.Add(plikOdUzytkownika);
            }

            await kontekst.SaveChangesAsync();
            return Ok(plikiOdUzytkownika);
        }

        [HttpPost("DoBazy")]
        public string DoBazy()
        {
            var sciezka = Path.Combine(iwhe.ContentRootPath, "wrzucone", "plik.csv");
            WorkBook workbook = WorkBook.LoadCSV(sciezka, fileFormat: ExcelFileFormat.XLSX, ListDelimiter: ",");
            WorkSheet worksheet = workbook.DefaultWorkSheet;
            DataTable datatable = worksheet.ToDataTable(true);

            int licznik = -1;
            string linijka;
            TextReader reader = new StreamReader(sciezka);
            while ((linijka = reader.ReadLine()) != null) licznik++;
            reader.Close();

            using (SqlConnection polaczenie = new SqlConnection(@"Data Source=localhost; Initial Catalog=panstwa; Integrated Security=True"))
            {
                polaczenie.Open();
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(polaczenie))
                {
                    foreach (DataColumn datacolumn in datatable.Columns)
                        sqlbulkcopy.ColumnMappings.Add(datacolumn.ColumnName, datacolumn.ColumnName);
                    
                    sqlbulkcopy.DestinationTableName = "Panstwo";
                    
                    try
                    {
                        sqlbulkcopy.WriteToServer(datatable);
                    }
                    catch (Exception error)
                    {
                        polaczenie.Close();
                        return "Aktualizacja danych nie powiodła się – błąd zapisu danych z pliku do bazy.";
                    }

                    string tekstLicznika = licznik.ToString();
                    polaczenie.Close();
                    return "Aktualizacja danych powiodła się, dodano następującą liczbę wierszy: "+tekstLicznika;
                }
            }
        }
    }
}