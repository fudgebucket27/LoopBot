using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    /// <summary>
    /// The pending status
    /// </summary>
    public class Pending
    {
        /// <summary>
        /// Withdraw status
        /// </summary>
        [JsonProperty("withdraw")]
        public string? Withdraw { get; set; }

        /// <summary>
        /// Deposit status
        /// </summary>
        [JsonProperty("deposit")]
        public string? Deposit { get; set; }

    }

    /// <summary>
    /// The nft balance API response
    /// </summary>
    public class NftBalance
    {
        /// <summary>
        /// The total number of nfts held
        /// </summary>
        [JsonProperty("totalNum")]
        public int TotalNum { get; set; }
        /// <summary>
        /// The nft data info
        /// </summary>
        [JsonProperty("data")]
        public List<Datum>? Data { get; set; }
    }

    /// <summary>
    /// The nft token balance
    /// </summary>
    public class CoinBalance
    {
        /// <summary>
        /// The account id
        /// </summary>
        [JsonProperty("accountId")]
        public int AccountId { get; set; }
        /// <summary>
        /// The token id
        /// </summary>
        [JsonProperty("tokenId")]
        public int TokenId { get; set; }
        /// <summary>
        /// The total
        /// </summary>
        [JsonProperty("total")]
        public string? Total { get; set; }
        /// <summary>
        /// If locked
        /// </summary>
        [JsonProperty("locked")]
        public string? Locked { get; set; }
        /// <summary>
        /// The pending status
        /// </summary>
        [JsonProperty("pending")]
        public Pending? Pending { get; set; }
    }
    /// <summary>
    /// The nft base information
    /// </summary>
    public class NftBase
    {
        /// <summary>
        /// The name
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
        /// <summary>
        /// The decimals
        /// </summary>
        [JsonProperty("decimals")]
        public int Decimals { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Image url
        /// </summary>
        [JsonProperty("image")]
        public string? Image { get; set; }
        /// <summary>
        /// Properties
        /// </summary>
        [JsonProperty("properties")]
        public string? Properties { get; set; }
        /// <summary>
        /// Localization
        /// </summary>
        [JsonProperty("localization")]
        public string? Localization { get; set; }
        /// <summary>
        /// When it was created
        /// </summary>
        [JsonProperty("createdAt")]
        public long CreatedAt { get; set; }
        /// <summary>
        /// When it was updated
        /// </summary>
        [JsonProperty("updatedAt")]
        public long UpdatedAt { get; set; }
    }
    /// <summary>
    /// Cached info
    /// </summary>
    public class Cached
    {
        /// <summary>
        /// The avatar
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }
        /// <summary>
        /// The banner
        /// </summary>
        [JsonProperty("banner")]
        public string? Banner { get; set; }
        /// <summary>
        /// Tile uri
        /// </summary>
        [JsonProperty("tileUri")]
        public string? TileUri { get; set; }
        /// <summary>
        /// Thumbnail
        /// </summary>
        [JsonProperty("thumbnail")]
        public string? Thumbnail { get; set; }
    }
    /// <summary>
    /// Collection info
    /// </summary>
    public class CollectionInfo
    {
        /// <summary>
        /// The Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// The owner
        /// </summary>
        [JsonProperty("owner")]
        public string? Owner { get; set; }
        /// <summary>
        /// The name
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
        /// <summary>
        /// The contract address
        /// </summary>
        [JsonProperty("contractAddress")]
        public string? ContractAddress { get; set; }
        /// <summary>
        /// The collection address
        /// </summary>
        [JsonProperty("collectionAddress")]
        public string? CollectionAddress { get; set; }
        /// <summary>
        /// The base uri
        /// </summary>
        [JsonProperty("baseUri")]
        public string? BaseUri { get; set; }
        /// <summary>
        /// The nft factory
        /// </summary>
        [JsonProperty("nftFactory")]
        public string? NftFactory { get; set; }
        /// <summary>
        /// The description
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }
        /// <summary>
        /// The avatar
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }
        /// <summary>
        /// The banner
        /// </summary>
        [JsonProperty("banner")]
        public string? Banner { get; set; }
        /// <summary>
        /// The thumbnail
        /// </summary>
        [JsonProperty("thumbnail")]
        public string? Thumbnail { get; set; }
        /// <summary>
        /// The tile uri
        /// </summary>
        [JsonProperty("tileUri")]
        public string? TileUri { get; set; }
        /// <summary>
        /// Cached info
        /// </summary>
        [JsonProperty("cached")]
        public Cached? Cached { get; set; }
        /// <summary>
        /// The deployment status on Layer 1
        /// </summary>
        [JsonProperty("deployStatus")]
        public string? DeployStatus { get; set; }
        /// <summary>
        /// The nft type
        /// </summary>
        [JsonProperty("nftType")]
        public string? NftType { get; set; }
        /// <summary>
        /// The times info
        /// </summary>
        [JsonProperty("times")]
        public Times? Times { get; set; }
        /// <summary>
        /// The extra info
        /// </summary>
        [JsonProperty("extra")]
        public Extra? Extra { get; set; }
    }
    /// <summary>
    /// Data about the nft
    /// </summary>
    public class Datum
    {
        /// <summary>
        /// The id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// The account id
        /// </summary>
        [JsonProperty("accountId")]
        public int AccountId { get; set; }
        /// <summary>
        /// The token id
        /// </summary>
        [JsonProperty("tokenId")]
        public int TokenId { get; set; }
        /// <summary>
        /// The nft data, unique to the nft on Loopring
        /// </summary>
        [JsonProperty("nftData")]
        public string? NftData { get; set; }
        /// <summary>
        /// The token address
        /// </summary>
        [JsonProperty("tokenAddress")]
        public string? TokenAddress { get; set; }
        /// <summary>
        /// The nft id, not uniqiue, the same CID minted by different minters will have the same nft id
        /// </summary>
        [JsonProperty("nftId")]
        public string? NftId { get; set; }
        /// <summary>
        /// The nft type
        /// </summary>
        [JsonProperty("nftType")]
        public string? NftType { get; set; }
        /// <summary>
        /// The total held
        /// </summary>
        [JsonProperty("total")]
        public string? Total { get; set; }
        /// <summary>
        /// The amount locked
        /// </summary>
        [JsonProperty("locked")]
        public string? Locked { get; set; }
        /// <summary>
        /// The pending info
        /// </summary>
        [JsonProperty("pending")]
        public Pending? Pending { get; set; }
        /// <summary>
        /// The deployment status on layer 1
        /// </summary>
        [JsonProperty("deploymentStatus")]
        public string? DeploymentStatus { get; set; }
        /// <summary>
        /// Is it counterfactual?
        /// </summary>
        [JsonProperty("isCounterFactualNFT")]
        public bool IsCounterFactualNFT { get; set; }
        /// <summary>
        /// The metadata info
        /// </summary>
        [JsonProperty("metadata")]
        public Metadata? Metadata { get; set; }
        /// <summary>
        /// The minter
        /// </summary>
        [JsonProperty("minter")]
        public string? Minter { get; set; }
        /// <summary>
        /// The royalty percentage
        /// </summary>
        [JsonProperty("royaltyPercentage")]
        public int? RoyaltyPercentage { get; set; }
        /// <summary>
        /// Preference info
        /// </summary>
        [JsonProperty("preference")]
        public Preference? Preference { get; set; }
        /// <summary>
        /// Collection info
        /// </summary>
        [JsonProperty("collectionInfo")]
        public CollectionInfo? CollectionInfo { get; set; }
        /// <summary>
        /// When it was updated
        /// </summary>
        [JsonProperty("updatedAt")]
        public long UpdatedAt { get; set; }
        /// <summary>
        /// When the balance was updated
        /// </summary>
        [JsonProperty("balanceUpdatedAt")]
        public long BalanceUpdatedAt { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Datum other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    /// <summary>
    /// Extra info
    /// </summary>
    public class Extra
    {
        /// <summary>
        /// The image data
        /// </summary>
        [JsonProperty("imageData")]
        public string? ImageData { get; set; }
        /// <summary>
        /// The external url
        /// </summary>
        [JsonProperty("externalUrl")]
        public string? ExternalUrl { get; set; }
        /// <summary>
        /// The attributes
        /// </summary>
        [JsonProperty("attributes")]
        public string? Attributes { get; set; }
        /// <summary>
        /// The background color
        /// </summary>
        [JsonProperty("backgroundColor")]
        public string? BackgroundColor { get; set; }
        /// <summary>
        /// The animation url
        /// </summary>
        [JsonProperty("animationUrl")]
        public string? AnimationUrl { get; set; }
        /// <summary>
        /// The youtube url
        /// </summary>
        [JsonProperty("youtubeUrl")]
        public string? YoutubeUrl { get; set; }
        /// <summary>
        /// The minter
        /// </summary>
        [JsonProperty("minter")]
        public string? Minter { get; set; }
        /// <summary>
        /// The properties
        /// </summary>
        [JsonProperty("properties")]
        public Properties? Properties { get; set; }
        /// <summary>
        /// The mint channel
        /// </summary>
        [JsonProperty("mintChannel")]
        public string? MintChannel { get; set; }
    }
    /// <summary>
    /// Image size info
    /// </summary>
    public class ImageSize
    {
        /// <summary>
        /// 240x240
        /// </summary>
        [JsonProperty("240-240")]
        public string? Size240 { get; set; }
        /// <summary>
        /// 332x322
        /// </summary>
        [JsonProperty("332-332")]
        public string? Size332 { get; set; }
        /// <summary>
        /// Original size
        /// </summary>
        [JsonProperty("original")]
        public string? Original { get; set; }
    }
    /// <summary>
    /// Metadata info
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// The uri
        /// </summary>
        [JsonProperty("uri")]
        public string? Uri { get; set; }
        /// <summary>
        /// The nft base info
        /// </summary>
        [JsonProperty("base")]
        public NftBase? Base { get; set; }
        /// <summary>
        /// Image Size info
        /// </summary>
        [JsonProperty("imageSize")]
        public ImageSize? ImageSize { get; set; }
        /// <summary>
        /// Extra info
        /// </summary>
        [JsonProperty("extra")]
        public Extra? Extra { get; set; }
        /// <summary>
        /// The status
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }
        /// <summary>
        /// The nft type
        /// </summary>
        [JsonProperty("nftType")]
        public int NftType { get; set; }
        /// <summary>
        /// The network
        /// </summary>
        [JsonProperty("network")]
        public int Network { get; set; }
        /// <summary>
        /// The token address
        /// </summary>
        [JsonProperty("tokenAddress")]
        public string? TokenAddress { get; set; }
        /// <summary>
        /// The nft id
        /// </summary>
        [JsonProperty("nftId")]
        public string? NftId { get; set; }
    }
    /// <summary>
    /// Preference info
    /// </summary>
    public class Preference
    {
        /// <summary>
        /// If it's favourited
        /// </summary>
        [JsonProperty("favourite")]
        public bool Favourite { get; set; }
        /// <summary>
        /// If to hide
        /// </summary>
        [JsonProperty("hide")]
        public bool Hide { get; set; }
    }
    /// <summary>
    /// Properties info
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// If legacy
        /// </summary>
        [JsonProperty("isLegacy")]
        public bool IsLegacy { get; set; }
        /// <summary>
        /// If public
        /// </summary>
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }
        /// <summary>
        /// If counterfactual
        /// </summary>
        [JsonProperty("isCounterFactualNFT")]
        public bool IsCounterFactualNFT { get; set; }
        /// <summary>
        /// If mintable
        /// </summary>
        [JsonProperty("isMintable")]
        public bool IsMintable { get; set; }
        /// <summary>
        /// If editable
        /// </summary>
        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }
        /// <summary>
        /// If deletable
        /// </summary>
        [JsonProperty("isDeletable")]
        public bool IsDeletable { get; set; }
    }
    /// <summary>
    /// Times info
    /// </summary>
    public class Times
    {
        /// <summary>
        /// When it was created
        /// </summary>
        [JsonProperty("createdAt")]
        public long CreatedAt { get; set; }
        /// <summary>
        /// When it was updated
        /// </summary>
        [JsonProperty("updatedAt")]
        public long UpdatedAt { get; set; }
    }
}
