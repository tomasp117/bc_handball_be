using bc_handball_be.API.DTOs.Persons;

﻿namespace bc_handball_be.API.DTOs.Referees
{
    public class RefereeDTO : PersonDTO
    {
        public char License { get; set; }
    }
}
