namespace CaritasBack.Models
{
    public class OfertaViewModel
    {
        public int idOffer { get; set; }
        public int offerState { get; set; }
        // información del usuario que recibió la oferta y su publicación
        public int idUserOwner { get; set; }
        public string nameUserOwner { get; set; }
        public string surnameUserOwner { get; set; }
        public string profilePicUserOwner { get; set; }
        public int idPostOwner { get; set; }
        public string titlePostOwner { get; set; }
        public string descriptionPostOwner { get; set; }
        public string nameProductCategoriePostOwner { get; set; }
        public string nameProductStatePostOwner { get; set; }
        public string locationTradePostOwner { get; set; }
        public string imagePostOwner { get; set; }

        // información del usuario que hizo la oferta y su publicación
        public int idUserOffer { get; set; }
        public string nameUserOffer { get; set; }
        public string surnameUserOffer { get; set; }
        public string profilePicUserOffer { get; set; }
        public int idPostOffer { get; set; }
        public string titlePostOffer { get; set; }
        public string descriptionPostOffer { get; set; }
        public string nameProductCategoriePostOffer { get; set; }
        public string nameProductStatePostOffer { get; set; }
        public string locationTradePostOffer { get; set; }
        public string imagePostOffer { get; set; }

        // información que elegió el ofertante: centro, hora y fecha elegida para el intercambio
        public int idCenterPostChoosedTrade { get; set; }
        public string nameCenterPostChoosedTrade { get; set; }
        public string addressCenterPostChoosedTrade { get; set; }
        public string hourCenterPostChoosedTrade { get; set; }
        public string dateCenterPostChoosedTrade { get; set; }

        // id del centro (raw)
        public int idRawCenterPostChoosed { get; set; }
        public string locationTradeCenterChoosed { get; set; }

    }

    public class OfertasInvolucradas
    {
        public List<OfertaViewModel> ofertasRecibidas { get; set; }
        public List<OfertaViewModel> ofertasRealizadas { get; set; }
    }
}
