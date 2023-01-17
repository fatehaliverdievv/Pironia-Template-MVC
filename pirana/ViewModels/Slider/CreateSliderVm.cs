namespace pirana.ViewModels
{
    public class CreateSliderVm
    {
        public string SecondaryTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonName { get; set; }
        public IFormFile Image { get; set; }
        public int Order { get; set; }
    }
}
