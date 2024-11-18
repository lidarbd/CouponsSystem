using static Azure.Core.HttpHeader;

namespace CouponsSystem.Models
{
    public class CouponsManager
    {
        private Dictionary<string, Coupons> coupons = new Dictionary<string, Coupons>();


        //TODO what about all the items we pass, how do we initialize coupon
        public void Insert(string description, string code, string userCreatorID, DateTime createdDateTime, bool isPercentage, double discount, bool isMultipleDiscounts, int? maxUsageCount = null, DateTime? expirationDate = null)
        {
            // Check if the coupon code already exists in the system
            if (coupons.ContainsKey(code))
            {
                throw new InvalidOperationException("A coupon with this code already exists.");
            }

            //TODO
            Coupons coupon = new Coupons();
            //Coupon coupon = new Coupon(description, code, userCreatorID, createdDateTime, isPercentage, discount, isMultipleDiscounts, maxUsageCount, expirationDate);

            // Insert the coupon into the dictionary
            coupons.Add(coupon.Code, coupon);
        }

        public bool Delete(string couponCode)
        {
            if (coupons.ContainsKey(couponCode))
            {
                coupons.Remove(couponCode);
                return true; // Successfully deleted coupon
            }
            return false; // Coupon not found
        }

        public bool Update(string couponCode, Coupons updatedCoupon)
        {
            if (updatedCoupon == null)
            {
                throw new ArgumentNullException(nameof(updatedCoupon), "Updated coupon cannot be null.");
            }

            if (!coupons.ContainsKey(couponCode))
            {
                return false; // Coupon not found, cannot update
            }

            // Update the coupon details in the dictionary
            coupons[couponCode] = updatedCoupon;
            return true;
        }

        // Get a coupon by its code
        public Coupons Get(string couponCode)
        {
            if (coupons.ContainsKey(couponCode))
            {
                return coupons[couponCode];
            }
            return null; // Coupon not found
        }

        // Retrieve all coupons
        public List<Coupons> GetAllCoupons()
        {
            return new List<Coupons>(coupons.Values);
        }
    }
}
