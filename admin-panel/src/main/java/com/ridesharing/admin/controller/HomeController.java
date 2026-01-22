package com.ridesharing.admin.controller;

import com.ridesharing.admin.repository.DisputeRepository;
import com.ridesharing.admin.repository.RideRepository;
import com.ridesharing.admin.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class HomeController {
    
    @Autowired
    private UserRepository userRepository;
    
    @Autowired
    private RideRepository rideRepository;
    
    @Autowired
    private DisputeRepository disputeRepository;
    
    @GetMapping("/")
    public String home(Model model) {
        model.addAttribute("currentPage", "dashboard");
        
        // User stats
        long totalUsers = userRepository.count();
        long activeDrivers = userRepository.findByRole("Driver").stream()
                .filter(u -> Boolean.TRUE.equals(u.getIsDriverApproved()))
                .count();
        var pendingDrivers = userRepository.findByIsDriverApprovedFalse();
        
        // Ride stats
        long totalRides = rideRepository.count();
        var recentRides = rideRepository.findTop10ByOrderByCreatedAtDesc();
        var totalRevenue = rideRepository.getTotalRevenue();
        
        // Dispute stats
        long pendingDisputes = disputeRepository.findByStatus("Pending").size();
        
        model.addAttribute("totalUsers", totalUsers);
        model.addAttribute("activeDrivers", activeDrivers);
        model.addAttribute("totalRides", totalRides);
        model.addAttribute("pendingDisputes", pendingDisputes);
        model.addAttribute("recentRides", recentRides);
        model.addAttribute("pendingDrivers", pendingDrivers);
        model.addAttribute("totalRevenue", totalRevenue != null ? totalRevenue : java.math.BigDecimal.ZERO);
        
        return "dashboard";
    }
}
