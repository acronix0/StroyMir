using Microsoft.AspNetCore.Http;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SimpleShop.Service.Services
{
    public class ImportManager
    {
        private IRepositoryManager _repository;
        public List<Category> categories = new List<Category>();
        public List<Product> products = new List<Product>();
        public ImportManager(IRepositoryManager repositoryManager)
        {
            _repository = repositoryManager;
        }
        static string ChangeFileExtensionToWebp(string filePath)
        {
            if(filePath == null || filePath == "" || filePath == string.Empty)
                return string.Empty;
            try
            {
                string newFilePath = Path.Combine(Path.GetDirectoryName(filePath),
                                           $"{Path.GetFileNameWithoutExtension(filePath)}.webp");
                return newFilePath;
            }
            catch (Exception e )
            {
                var x = e.Message;
                return string.Empty;
            }
            
            
        }
        public async Task ImportXml(string xmlData)
        {
            categories.Clear();
            products.Clear();//
            
            var XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(xmlData);
            var importCategories = XmlDoc.GetElementsByTagName("Группа");
            foreach (XmlNode category in importCategories)
            {
                var name = category.ChildNodes.Cast<XmlNode>().First(n => n.Name == "Наименование").InnerText;
                string nameWithoutNumber = Regex.Replace(name, @"^\d+(\.\d+)*\s+", "");
                var article = category.ChildNodes.Cast<XmlNode>().First(n => n.Name == "Ид").InnerText;
                categories.Add(new Category()
                {
                    Article = article,
                    Name = nameWithoutNumber,
                    Image = "/"+article+"_webp"+".webp",
                });
            }
            var getCategories = await _repository.CategoryRepository.GetCategories();
            var oldCategories = getCategories.ToList();
            var newCategories = new List<Category>();
            var needChangeCategories = new List<Category>();
            foreach (var item in categories)
            {
                var oldCategory = oldCategories.FirstOrDefault(c => c.Article == item.Article);
                if (oldCategory != null)
                {
                    if (!oldCategory.Equals(item))
                    {
                        item.Id = oldCategory.Id;
                        needChangeCategories.Add(item);
                    }
                    continue;
                }
                newCategories.Add(item);
            }

            if (newCategories.Any())
                await _repository.CategoryRepository.AddRangeCategory(newCategories);

            if (needChangeCategories.Any())
                await _repository.CategoryRepository.UpdateRangeCategory(needChangeCategories);

            await _repository.SaveAsync();

            var query = await _repository.CategoryRepository.GetCategories();
            categories = query.ToList();



            var importProducts = XmlDoc.GetElementsByTagName("Товар");
            var i = 0;
            foreach (XmlNode product in importProducts)
            {
                i++;
                var name = "";
                var article = "";
                var image = "";
                var categoryId = "";
                var textImage = "";
                try
                {
                    name = product.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Наименование").InnerText;
                    article = product.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Ид").InnerText;
                    
                    categoryId = product.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Группы")
                                            .ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Ид").InnerText;
                    textImage = product.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Картинка").InnerText;
                    image = textImage==""?"": textImage.Replace("import_files", "").Replace('.', '_') + ".webp";
                }
                catch (Exception e )
                {

                    var err = e.Message;
                }
                try
                {
                    if(categoryId != "")
                    {
                        products.Add(new Product()
                        {
                            Article = article,
                            Name = name,
                            Image = image,
                            Category = categories.First(c => c.Article == categoryId),
                            Count = 0,
                        });
                    }
                   
                }
                catch (Exception e)
                {

                    var x = e.Message;
                }
                
            }


          
        }


        public async Task OffersXml(string xmlData)
        {

            
            var XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(xmlData);
            var importProducts = XmlDoc.GetElementsByTagName("Предложение");

            XDocument doc = XDocument.Parse(xmlData);
           
             foreach (XmlNode importProduct in importProducts)
            {

                var article = "";
                var price = "0";
                var count = "0";
                //XElement importProductElement = XElement.Parse(importProduct.OuterXml);
                try
                {
                    
                    article = importProduct.ChildNodes.Cast<XmlNode>().First(n => n.Name == "Артикул").InnerText;
                    count = importProduct.ChildNodes.Cast<XmlNode>().First(n => n.Name == "Количество").InnerText;
                    price = importProduct.ChildNodes.Cast<XmlNode>().First(n => n.Name == "Цены")
                                        .ChildNodes.Cast<XmlNode>().First(n => n.Name == "Цена")
                                        .ChildNodes.Cast<XmlNode>().First(n => n.Name == "ЦенаЗаЕдиницу").InnerText;
                    
                }
                catch (Exception e )
                {
                   var err = e.Message;
                }
               
                //change in prList
                var product = products.FirstOrDefault(p => p.Article == article);
                if (product!=null)
                {
                    try
                    {
                        if (decimal.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalPrice))
                        {
                            product.Price = decimalPrice;
                        }
                        product.Count = Int32.Parse(count);
                    }
                    catch (Exception e )
                    {

                        var err = e.Message;
                    }
                    

                }
            }

            var getProducts = await _repository.ProductRepository.GetProducts();
            var oldProducts = getProducts.ToList();
            var newProducts = new List<Product>();
            var needChangeProducts = new List<Product>();
            foreach (var item in products)
            {
                var oldProduct = oldProducts.FirstOrDefault(c => c.Article == item.Article);
                if (oldProduct != null)
                {
                    if (!oldProduct.Equals(item))
                    {
                        item.Id = oldProduct.Id;
                        needChangeProducts.Add(item);
                    }
                    continue;
                }
                newProducts.Add(item);
            }

            if (newProducts.Any())
                await _repository.ProductRepository.AddRangeProduct(newProducts);

            if (needChangeProducts.Any())
                await _repository.ProductRepository.UpdateRangeProduct(needChangeProducts);

            await _repository.SaveAsync();

            var query = await _repository.ProductRepository.GetProducts();
            products = query.ToList();
        }


        public async Task ImportImage(IFormFile file)
        {

        }


    }
}
