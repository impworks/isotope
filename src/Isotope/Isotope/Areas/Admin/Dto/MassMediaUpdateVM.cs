namespace Isotope.Areas.Admin.Dto;

public class MassMediaUpdateVM : MassMediaActionVM
{
    public Option<string> Description { get; set; }
    public Option<string> Date { get; set; }
}

public class Option<T>
{
    public bool IsSet { get; set; }
    public T Value { get; set; }
}