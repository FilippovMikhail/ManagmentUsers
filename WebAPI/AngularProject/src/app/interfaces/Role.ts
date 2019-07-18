export interface Role{
    Id?: string;
    RoleName: string;
    DisplayName: string;
    CanCreate: boolean;
    CanEdit: boolean;
    CanShow: boolean;
    CanPrint: boolean;
}