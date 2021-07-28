using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SeanBruceMiddleTier.Data;
using SeanBruceMiddleTier.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeanBruceMiddleTier.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            var path = Path.Combine(_env.ContentRootPath, String.Format("Data/Source/SalesOrders.xlsx"));
            //using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //using (var ep = new ExcelPackage(stream))
                using (var ep = package)
                {
                    // get the first worksheet
                    var ws = ep.Workbook.Worksheets[0];
                    // initialize the record counters
                    var nOrders = 0;
                    var nPeoples = 0;
                    var nItems = 0;

                    #region Import all People
                    var lstPerson = _context.Peoples.ToList();

                    for (int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
                    {
                        var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                        var lastName = row[nRow, 3].GetValue<string>();

                        if (lstPerson.Where(p => p.LastName == lastName).Count() == 0)
                        {
                            var person = new People();
                            person.LastName = lastName;
                            _context.Peoples.Add(person);
                            await _context.SaveChangesAsync();
                            lstPerson.Add(person);
                            nPeoples++;
                        }
                    }
                    #endregion

                    #region Import all Items
                    var lstItem = _context.Items.ToList();

                    for (int nRow = 2; nRow < ws.Dimension.End.Row; nRow++)
                    {
                        var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                        var name = row[nRow, 4].GetValue<string>();

                        if (lstItem.Where(i => i.Name == name).Count() == 0)
                        {
                            var item = new Item() {
                                Name = name
                            };
                            _context.Items.Add(item);
                            await _context.SaveChangesAsync();
                            lstItem.Add(item);
                            nItems++;
                        }
                    }
                    #endregion

                    #region Import all Countries
                    //// create a list containing all the countries already existing into the Database (it will be empty on first run).
                    //var lstCountries = _context.Countries.ToList();

                    //// iterates through all rows, skipping the first one
                    //for (int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
                    //{
                    //    var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                    //    var name = row[nRow, 5].GetValue<string>();
                    //    // Did we already created a country with that name?
                    //    if (lstCountries.Where(c => c.Name == name).Count() == 0)
                    //    {
                    //        // create the Country entity and fill it with xlsx data
                    //        var country = new Country();
                    //        country.Name = name;
                    //        country.ISO2 = row[nRow, 6].GetValue<string>();
                    //        country.ISO3 = row[nRow, 7].GetValue<string>();
                    //        // save it into the Database
                    //        _context.Countries.Add(country);
                    //        await _context.SaveChangesAsync();
                    //        // store the country to retrieve
                    //        // its Id later on
                    //        lstCountries.Add(country);
                    //        // increment the counter
                    //        nCountries++;
                    //    }
                    //}
                    #endregion

                    #region Import all Cities
                    //// iterates through all rows, skipping the first one
                    //for (int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
                    //{
                    //    var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                    //    // create the City entity and fill it with xlsx data
                    //    var city = new City();
                    //    city.Name = row[nRow, 1].GetValue<string>();
                    //    city.Name_ASCII = row[nRow, 2].GetValue<string>();
                    //    city.Lat = row[nRow, 3].GetValue<decimal>();
                    //    city.Lon = row[nRow, 4].GetValue<decimal>();
                    //    // retrieve CountryId
                    //    var countryName = row[nRow, 5].GetValue<string>();
                    //    var country = lstCountries.Where(c => c.Name == countryName).FirstOrDefault();
                    //    city.CountryId = country.Id;
                    //    // save the city into the Database
                    //    _context.Cities.Add(city);
                    //    await _context.SaveChangesAsync();
                    //    // increment the counter
                    //    nCities++;
                    //}
                    #endregion

                    #region Import all Orders
                    for (int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
                    {
                        var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                        var order = new Order()
                        {
                            OrderDate = row[nRow, 1].GetValue<DateTime>(),
                            Region = row[nRow, 2].GetValue<string>(),
                            Quantity = row[nRow, 5].GetValue<int>(),
                            UnitCost = row[nRow, 6].GetValue<decimal>(),
                            Total = row[nRow, 7].GetValue<decimal>()
                        };
                        var personName = row[nRow, 3].GetValue<string>();
                        var person = lstPerson.Where(p => p.LastName == personName).FirstOrDefault();
                        order.PeopleId = person.Id;

                        var itemName = row[nRow, 4].GetValue<string>();
                        var item = lstItem.Where(i => i.Name == itemName).FirstOrDefault();
                        order.ItemId = item.Id;

                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        nOrders++;
                    }
                    #endregion

                    return new JsonResult(new
                    {
                        Peoples = nPeoples,
                        Items = nItems,
                        Orders = nOrders,
                    });
                }
            }
        }
    }
}
