using PaymentService.Model;

namespace PaymentService{
    
    class MenuClient {

        private HttpClient _httpClient;

        public MenuClient(HttpClient httpClient){
            _httpClient = httpClient;
        }

        public async Task<MenuDTO> GetMenu (string menuId){
            
            return await _httpClient.GetFromJsonAsync<MenuDTO>($"/menu/{menuId}");
        }
    }
}