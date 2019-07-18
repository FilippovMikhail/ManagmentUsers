export class UserAndRole{
    constructor(public UserName: string,
    public IncludedRoleNames: string[],
    public ExcludedRoleNames: string[]) {}
  }