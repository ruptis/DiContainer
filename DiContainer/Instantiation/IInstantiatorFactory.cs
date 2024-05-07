using DiContainer.Registrations;
namespace DiContainer.Instantiation;

public interface IInstantiatorFactory
{
    IInstantiator CreateInstantiator(RegistrationBuilder registrationBuilder);
}
