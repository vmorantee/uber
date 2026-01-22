package com.ridesharing.admin.controller;

import com.ridesharing.admin.entity.Dispute;
import com.ridesharing.admin.entity.Ride;
import com.ridesharing.admin.entity.Wallet;
import com.ridesharing.admin.entity.WalletTransaction;
import com.ridesharing.admin.repository.DisputeRepository;
import com.ridesharing.admin.repository.RideRepository;
import com.ridesharing.admin.repository.WalletRepository;
import com.ridesharing.admin.repository.WalletTransactionRepository;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

@Controller
@RequestMapping("/disputes")
public class DisputeController {
    
    private final DisputeRepository disputeRepository;
    private final WalletRepository walletRepository;
    private final WalletTransactionRepository transactionRepository;
    private final RideRepository rideRepository;
    
    public DisputeController(DisputeRepository disputeRepository, 
                           WalletRepository walletRepository,
                           WalletTransactionRepository transactionRepository,
                           RideRepository rideRepository) {
        this.disputeRepository = disputeRepository;
        this.walletRepository = walletRepository;
        this.transactionRepository = transactionRepository;
        this.rideRepository = rideRepository;
    }
    
    @GetMapping
    public String listDisputes(Model model) {
        model.addAttribute("currentPage", "disputes");
        List<Dispute> disputes = disputeRepository.findAll();
        long openCount = disputes.stream().filter(d -> "Pending".equals(d.getStatus()) || "OPEN".equals(d.getStatus())).count();
        long resolvedCount = disputes.stream().filter(d -> "Resolved".equals(d.getStatus()) || "RESOLVED".equals(d.getStatus())).count();
        
        model.addAttribute("disputes", disputes);
        model.addAttribute("openCount", openCount);
        model.addAttribute("resolvedCount", resolvedCount);
        return "disputes";
    }
    
    @GetMapping("/{id}")
    public String viewDispute(@PathVariable Integer id, Model model) {
        model.addAttribute("currentPage", "disputes");
        Dispute dispute = disputeRepository.findById(id)
            .orElseThrow(() -> new RuntimeException("Dispute not found"));
        
        Ride ride = rideRepository.findById(dispute.getRideId()).orElse(null);
        
        model.addAttribute("dispute", dispute);
        model.addAttribute("ride", ride);
        return "dispute-detail";
    }
    
    @PostMapping("/{id}/resolve")
    @Transactional
    public String resolveDispute(@PathVariable Integer id,
                                @RequestParam String decision,
                                @RequestParam BigDecimal refundAmount,
                                @RequestParam(required = false) String resolutionNotes) {
        
        Dispute dispute = disputeRepository.findById(id)
            .orElseThrow(() -> new RuntimeException("Dispute not found"));
        
        Ride ride = rideRepository.findById(dispute.getRideId())
            .orElseThrow(() -> new RuntimeException("Ride not found"));
        
        if (decision.equals("REFUND_PASSENGER") && refundAmount.compareTo(BigDecimal.ZERO) > 0) {
            Wallet passengerWallet = walletRepository.findByUserId(ride.getPassengerId())
                .orElseThrow(() -> new RuntimeException("Passenger wallet not found"));
            
            passengerWallet.setBalance(passengerWallet.getBalance().add(refundAmount));
            passengerWallet.setUpdatedAt(LocalDateTime.now());
            walletRepository.save(passengerWallet);
            
            WalletTransaction transaction = new WalletTransaction();
            transaction.setWalletId(passengerWallet.getId());
            transaction.setAmount(refundAmount);
            transaction.setType("Refund");
            transaction.setTransactionDate(LocalDateTime.now());
            transaction.setReferenceId(dispute.getRideId());
            transaction.setDescription("Refund for dispute #" + dispute.getId() + " - " + resolutionNotes);
            transactionRepository.save(transaction);
        }
        
        if (decision.equals("COMPENSATE_DRIVER") && refundAmount.compareTo(BigDecimal.ZERO) > 0) {
            Wallet driverWallet = walletRepository.findByUserId(ride.getDriverId())
                .orElseThrow(() -> new RuntimeException("Driver wallet not found"));
            
            driverWallet.setBalance(driverWallet.getBalance().add(refundAmount));
            driverWallet.setUpdatedAt(LocalDateTime.now());
            walletRepository.save(driverWallet);
            
            WalletTransaction transaction = new WalletTransaction();
            transaction.setWalletId(driverWallet.getId());
            transaction.setAmount(refundAmount);
            transaction.setType("Refund");
            transaction.setTransactionDate(LocalDateTime.now());
            transaction.setReferenceId(dispute.getRideId());
            transaction.setDescription("Compensation for dispute #" + dispute.getId() + " - " + resolutionNotes);
            transactionRepository.save(transaction);
        }
        
        dispute.setStatus("RESOLVED");
        dispute.setResolutionDecision(decision + ": " + resolutionNotes);
        dispute.setRefundAmount(refundAmount);
        dispute.setResolvedAt(LocalDateTime.now());
        disputeRepository.save(dispute);
        
        return "redirect:/disputes";
    }
}
