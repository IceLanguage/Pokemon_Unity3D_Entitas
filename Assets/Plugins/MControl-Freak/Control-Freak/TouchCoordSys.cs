
	// --------------------
	/// Touch Coordinate System.
	// --------------------
	 
	public enum TouchCoordSys
		{
		SCREEN_PX,			///< pixels, relative to screen's top-left corner
		SCREEN_NORMALIZED,	///< normalized screen coordinates - (0,0) in the top-left, (1,1) in the bottom-right corner
		SCREEN_CM,			///< centimeters, relative to screen's top-left corner
		SCREEN_INCH,		///< inches, relative to screen's top-left corner
		LOCAL_PX,			///< pixels, relative to control rectangle's top-left corner
		LOCAL_NORMALIZED,	///< control's normalized coords, relative to control rectangle's top-left corner
		LOCAL_CM,			///< centimeters, relative to control rectangle's top-left corner
		LOCAL_INCH			///< inches, relative to control rectangle's top-left corner
		}

