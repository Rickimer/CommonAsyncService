namespace CommonAsyncService.BLL.DTO
{
    public class CommonRezultDto
    {
        private Dictionary<ConsumingServicesDto, HealthCheckRezultsDto> _commonRezult = new Dictionary<ConsumingServicesDto, HealthCheckRezultsDto>();

        public Dictionary<ConsumingServicesDto, HealthCheckRezultsDto> CommonRezult
        {
            get
            {
                return _commonRezult;
            }
            set
            {
                _commonRezult = value;
            }
        }
    }
}
