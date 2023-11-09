using System;
using System.ComponentModel.DataAnnotations;

namespace src.Web.Common.Models.ParentViewModels
{
    public class ParenetInforViewModel
    {
        public int parent_id{get;set;}
        public string contact_type{get;set;}
        public string full_name{get;set;}
        public string address{get;set;}
        public string phone_number{get;set;}
        public string email{get;set;}
    }
}