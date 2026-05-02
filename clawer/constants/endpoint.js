const DETAIL_URL =
	"https://mp-search-api.tcgplayer.com/v2/product/[PRODUCT_ID]/details?mpfev=5106";

const SEARCH_URL =
	"https://mp-search-api.tcgplayer.com/v1/search/request?q=&isList=false&mpfev=5106";

const IMAGE_URL =
	"https://tcgplayer-cdn.tcgplayer.com/product/${PRODUCT_ID}_in_1000x1000.jpg";

module.exports = {
	DETAIL_URL,
	SEARCH_URL,
	IMAGE_URL
};
