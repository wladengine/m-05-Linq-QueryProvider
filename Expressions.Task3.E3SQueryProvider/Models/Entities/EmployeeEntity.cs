using System.Collections.Generic;
using Expressions.Task3.E3SQueryProvider.Attributes;

namespace Expressions.Task3.E3SQueryProvider.Models.Entities
{
    [E3SMetaType("meta:people-suite:people-api:com.epam.e3s.app.people.api.data.EmployeeEntity")]
    public class EmployeeEntity : BaseE3SEntity
    {
        private List<string> Phone;

        private SkillsEntity Skill { get; set; }

        private List<string> FirstName { get; set; }

        private List<string> LastName { get; set; }

        private List<string> FullName { get; set; }

        private List<string> Country { get; set; }

        private List<string> City { get; set; }

        private List<string> Email { get; set; }

        private List<string> Skype { get; set; }

        private List<string> Social { get; set; }

        public string Manager { get; set; }

        public string Superior { get; set; }

        public string StartWorkDate { get; set; }

        public string Unit { get; set; }

        public string Office { get; set; }

        public string Room { get; set; }

        public string Status { get; set; }

        public string Birthday { get; set; }

        public List<WorkHistoryEntity> WorkHistory { get; set; }

        private List<string> JobFunction { get; set; }

        private List<RecognitionEntity> Recognition { get; set; }

        private List<string> Badge { get; set; }

        public string EndProbationDate { get; set; }

        public string Endworkdate { get; set; }

        public string Workstation { get; set; }

        public string NativeName { get; set; }

        public double Billable { get; set; }

        public double NonBillable { get; set; }
    }
}
