using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Joo_Jeana_HW5.Models
{
    // The enum must be defined so the Product class knows what "ProductType" is
    public enum ProductType 
    { Hat, Pants, Sweatshirt, Tank, TShirt, Other}

    public class Product
    {
        public Int32 ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public String Name { get; set; }

        public String Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal Price { get; set; }

        public ProductType ProductType { get; set; }

        public List<Supplier>? Suppliers { get; set; } = new List<Supplier>();
        public List<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
    }
}