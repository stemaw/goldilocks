namespace CarChooser.Domain
{
    public interface ISearchCars
    {
        Car GetCar(Search search);
    }
}