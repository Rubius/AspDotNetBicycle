namespace Common.Models.Users;

public enum Permission
{
    BicycleCreate = 0,
    BicycleRead = 100,
    BicycleEdit = 200,
    BicycleDelete = 300,
    BicycleModelCreate = 400,
    BicycleModelRead = 500,
    BicycleModelEdit = 600,
    BicycleModelDelete = 700,
    ServiceManage = 800,
    RideCreate = 900,
    RideEdit = 1000,
    RideRead = 1100,
    AddressRead = 1200
}