using System;
using System.Collections.Generic;

namespace Registro.Models
{
    public class CarouselListModel
    {
        public List<GradeModel> items;
        
        public CarouselListModel(List<GradeModel> items)
        {
            this.items = items;
        }
    }
}
