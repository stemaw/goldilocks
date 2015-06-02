using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface IFilter
    {
        List<Car> Filter(List<Car> carOptions);
    }
}