const mongoose = require('mongoose');

const skuSchema = new mongoose.Schema({
    sku: { type: Number },
    condition: { type: String },
    variant: { type: String },
    language: { type: String }
}, { _id: false });

const itemSchema = new mongoose.Schema({
    productId: { type: Number, unique: true, required: true },
    productName: { type: String, index: true },
    productUrlName: { type: String },
    productLineName: { type: String, index: true },
    productLineId: { type: Number },
    productLineUrlName: { type: String },
    productTypeName: { type: String },
    productTypeId: { type: Number },
    rarityName: { type: String },
    sealed: { type: Boolean, default: false },
    marketPrice: { type: Number },
    lowestPrice: { type: Number },
    medianPrice: { type: Number },
    lowestPriceWithShipping: { type: Number },
    customListings: { type: Number },
    shippingCategoryId: { type: Number },
    duplicate: { type: Boolean, default: false },
    setId: { type: Number, index: true },
    setCode: { type: String, index: true },
    setName: { type: String, index: true },
    setUrlName: { type: String },
    imageCount: { type: Number },
    score: { type: Number },
    sellers: { type: Number },
    foilOnly: { type: Boolean, default: false },
    sellerListable: { type: Boolean, default: true },
    productStatusId: { type: Number },
    maxFulfillableQuantity: { type: Number },
    normalOnly: { type: Boolean, default: false },
    listings: { type: Number },
    customAttributes: { type: mongoose.Schema.Types.Mixed },
    formattedAttributes: { type: mongoose.Schema.Types.Mixed },
    skus: [skuSchema],
    imageUrl: { type: String },
    imageBase64: { type: String }
}, { 
    timestamps: true 
});

itemSchema.index({ productLineName: 1, setName: 1 });

const Item = mongoose.model('Item', itemSchema);

module.exports = Item;
