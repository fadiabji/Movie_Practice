using Movie_Exercise.Models;
using Movie_Exercise.Models.ViewModels;
using AutoMapper;


namespace Movie_Exercise.MapperHelper
{
    public class MappingHelper : Profile
    {
        public MappingHelper()  
        {
            // the order important here
            //CreateMap<AddArticleVM, Article>();
            CreateMap<Customer, Customer>();
           
        }
    }
}
